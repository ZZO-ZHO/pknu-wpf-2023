using ControlzEx.Standard;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using CodingTest.Logics;
using CodingTest.Models;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Media.Media3D;

namespace CodingTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string serviceKey = "%2FNtx3GoQdoc%2BwaMK%2BX2zeEch5OYmBCAs4wQ74iRnhBGoIIOCyjtbhEszVrYlWbyGT4hBFa6WuPjb8%2F920%2BlHHA%3D%3D&";
            string openApiUri = $@"https://apis.data.go.kr/6260000/BusanCrsTrnngInfoService/getCrsTrnngInfo?serviceKey={serviceKey}pageNo=1&numOfRows=1000&resultType=json";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var resultCode = Convert.ToString(jsonResult["getCrsTrnngInfo"]["header"]["resultCode"]);

            try
            {
                if (resultCode == "00")
                {
                    var data = jsonResult["getCrsTrnngInfo"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var classList = new List<ClassList>();
                    foreach (var list in json_array)
                    {

                        classList.Add(new ClassList
                        {
                            Id = 0,
                            LctreNm = Convert.ToString(list["lctreNm"]),
                            ProgrsSttusNm = Convert.ToString(list["progrsSttusNm"]),
                            LctreBeginTime = Convert.ToString(list["lctreBeginTime"]),
                            LctreEndDttm = Convert.ToString(list["lctreEndDttm"]),
                            ReqstBeginDttm = Convert.ToString(list["reqstBeginDttm"]),
                            ReqstEndDttm = Convert.ToString(list["reqstEndDttm"]),
                            CanclUseAt = Convert.ToString(list["canclUseAt"]),
                            LctreChargeAmount = Convert.ToString(list["lctreChargeAmount"]),
                            LctrePosblDfk = Convert.ToString(list["lctrePosblDfk"]),
                            LctreBeginDttm = Convert.ToString(list["lctreBeginDttm"]),
                            LctreEndTime = Convert.ToString(list["lctreEndTime"]),
                            LctreRefrnc = Convert.ToString(list["lctreRefrnc"]),
                            FileOrginlNm = Convert.ToString(list["fileOrginlNm"]),
                            Gubun = Convert.ToString(list["gubun"]),
                            ResveGroupNm = Convert.ToString(list["resveGroupNm"]),
                            LctreResveMth = Convert.ToString(list["lctreResveMth"]),
                            LctrePsncpa = Convert.ToString(list["lctrePsncpa"]),
                            ApplyCnt = Convert.ToString(list["applyCnt"]),
                            AdresLo = Convert.ToString(list["adresLo"]),
                            AdresLa = Convert.ToString(list["adresLa"]),
                            Adres = Convert.ToString(list["adres"]),
                            ResidualCNT = Convert.ToString(list["residualCNT"])
                        });
                    }

                    this.DataContext = classList;  // 데이터 넘어오는지 확인
                    StsResult.Content = $"OpenAPI {classList.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }

            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT SUBSTRING(adres, 4, LOCATE(' ', adres, 3)) AS adr
                                  FROM classlist
                                  WHERE adres LIKE '%구%'
                                  group by 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["adr"]));
                }
                CboAddress.ItemsSource = saveDateList;
            }

            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT progrsSttusNm AS st
                                  FROM classlist
                                  group by 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["st"]));
                }
                CboState.ItemsSource = saveDateList;
            }



        }

        private async void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (CboAddress.SelectedValue != null && CboState.SelectedValue != null)
            {
                // MessageBox.Show(CboReqDate.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Id,
                                        LctreNm,
                                        ProgrsSttusNm,
                                        LctreBeginTime,
                                        LctreEndDttm,
                                        ReqstBeginDttm,
                                        ReqstEndDttm,
                                        CanclUseAt,
                                        LctreChargeAmount,
                                        LctrePosblDfk,
                                        LctreBeginDttm,
                                        LctreEndTime,
                                        LctreRefrnc,
                                        FileOrginlNm,
                                        Gubun,
                                        ResveGroupNm,
                                        LctreResveMth,
                                        LctrePsncpa,
                                        ApplyCnt,
                                        AdresLo,
                                        AdresLa,
                                        Adres,
                                        ResidualCNT
                                    FROM classlist
                                    WHERE SUBSTRING(adres, 4, LOCATE(' ', adres, 3)) = @Adres AND progrsSttusNm = @ProgrsSttusNm;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Adres", CboAddress.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@ProgrsSttusNm", CboState.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "classlist");
                    List<ClassList> classList = new List<ClassList>();
                    foreach (DataRow list in ds.Tables["classList"].Rows)
                    {
                        classList.Add(new ClassList
                        {
                            Id = 0,
                            LctreNm = Convert.ToString(list["lctreNm"]),
                            ProgrsSttusNm = Convert.ToString(list["progrsSttusNm"]),
                            LctreBeginTime = Convert.ToString(list["lctreBeginTime"]),
                            LctreEndDttm = Convert.ToString(list["lctreEndDttm"]),
                            ReqstBeginDttm = Convert.ToString(list["reqstBeginDttm"]),
                            ReqstEndDttm = Convert.ToString(list["reqstEndDttm"]),
                            CanclUseAt = Convert.ToString(list["canclUseAt"]),
                            LctreChargeAmount = Convert.ToString(list["lctreChargeAmount"]),
                            LctrePosblDfk = Convert.ToString(list["lctrePosblDfk"]),
                            LctreBeginDttm = Convert.ToString(list["lctreBeginDttm"]),
                            LctreEndTime = Convert.ToString(list["lctreEndTime"]),
                            LctreRefrnc = Convert.ToString(list["lctreRefrnc"]),
                            FileOrginlNm = Convert.ToString(list["fileOrginlNm"]),
                            Gubun = Convert.ToString(list["gubun"]),
                            ResveGroupNm = Convert.ToString(list["resveGroupNm"]),
                            LctreResveMth = Convert.ToString(list["lctreResveMth"]),
                            LctrePsncpa = Convert.ToString(list["lctrePsncpa"]),
                            ApplyCnt = Convert.ToString(list["applyCnt"]),
                            AdresLo = Convert.ToString(list["adresLo"]),
                            AdresLa = Convert.ToString(list["adresLa"]),
                            Adres = Convert.ToString(list["adres"]),
                            ResidualCNT = Convert.ToString(list["residualCNT"])
                        });
                    }

                    this.DataContext = classList;
                    StsResult.Content = $"OpenAPI {classList.Count}건 조회완료";
                }
            }
            else if (CboAddress.SelectedValue != null && CboState.SelectedValue == null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Id,
                                        LctreNm,
                                        ProgrsSttusNm,
                                        LctreBeginTime,
                                        LctreEndDttm,
                                        ReqstBeginDttm,
                                        ReqstEndDttm,
                                        CanclUseAt,
                                        LctreChargeAmount,
                                        LctrePosblDfk,
                                        LctreBeginDttm,
                                        LctreEndTime,
                                        LctreRefrnc,
                                        FileOrginlNm,
                                        Gubun,
                                        ResveGroupNm,
                                        LctreResveMth,
                                        LctrePsncpa,
                                        ApplyCnt,
                                        AdresLo,
                                        AdresLa,
                                        Adres,
                                        ResidualCNT
                                    FROM classlist
                                    WHERE SUBSTRING(adres, 4, LOCATE(' ', adres, 3)) = @Adres ";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Adres", CboAddress.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "classlist");
                    List<ClassList> classList = new List<ClassList>();
                    foreach (DataRow list in ds.Tables["classList"].Rows)
                    {
                        classList.Add(new ClassList
                        {
                            Id = 0,
                            LctreNm = Convert.ToString(list["lctreNm"]),
                            ProgrsSttusNm = Convert.ToString(list["progrsSttusNm"]),
                            LctreBeginTime = Convert.ToString(list["lctreBeginTime"]),
                            LctreEndDttm = Convert.ToString(list["lctreEndDttm"]),
                            ReqstBeginDttm = Convert.ToString(list["reqstBeginDttm"]),
                            ReqstEndDttm = Convert.ToString(list["reqstEndDttm"]),
                            CanclUseAt = Convert.ToString(list["canclUseAt"]),
                            LctreChargeAmount = Convert.ToString(list["lctreChargeAmount"]),
                            LctrePosblDfk = Convert.ToString(list["lctrePosblDfk"]),
                            LctreBeginDttm = Convert.ToString(list["lctreBeginDttm"]),
                            LctreEndTime = Convert.ToString(list["lctreEndTime"]),
                            LctreRefrnc = Convert.ToString(list["lctreRefrnc"]),
                            FileOrginlNm = Convert.ToString(list["fileOrginlNm"]),
                            Gubun = Convert.ToString(list["gubun"]),
                            ResveGroupNm = Convert.ToString(list["resveGroupNm"]),
                            LctreResveMth = Convert.ToString(list["lctreResveMth"]),
                            LctrePsncpa = Convert.ToString(list["lctrePsncpa"]),
                            ApplyCnt = Convert.ToString(list["applyCnt"]),
                            AdresLo = Convert.ToString(list["adresLo"]),
                            AdresLa = Convert.ToString(list["adresLa"]),
                            Adres = Convert.ToString(list["adres"]),
                            ResidualCNT = Convert.ToString(list["residualCNT"])
                        });
                    }

                    this.DataContext = classList;
                    StsResult.Content = $"OpenAPI {classList.Count}건 조회완료";
                }
            }
            else if (CboAddress.SelectedValue == null && CboState.SelectedValue != null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Id,
                                        LctreNm,
                                        ProgrsSttusNm,
                                        LctreBeginTime,
                                        LctreEndDttm,
                                        ReqstBeginDttm,
                                        ReqstEndDttm,
                                        CanclUseAt,
                                        LctreChargeAmount,
                                        LctrePosblDfk,
                                        LctreBeginDttm,
                                        LctreEndTime,
                                        LctreRefrnc,
                                        FileOrginlNm,
                                        Gubun,
                                        ResveGroupNm,
                                        LctreResveMth,
                                        LctrePsncpa,
                                        ApplyCnt,
                                        AdresLo,
                                        AdresLa,
                                        Adres,
                                        ResidualCNT
                                    FROM classlist
                                    WHERE progrsSttusNm = @ProgrsSttusNm;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProgrsSttusNm", CboState.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "classlist");
                    List<ClassList> classList = new List<ClassList>();
                    foreach (DataRow list in ds.Tables["classList"].Rows)
                    {
                        classList.Add(new ClassList
                        {
                            Id = 0,
                            LctreNm = Convert.ToString(list["lctreNm"]),
                            ProgrsSttusNm = Convert.ToString(list["progrsSttusNm"]),
                            LctreBeginTime = Convert.ToString(list["lctreBeginTime"]),
                            LctreEndDttm = Convert.ToString(list["lctreEndDttm"]),
                            ReqstBeginDttm = Convert.ToString(list["reqstBeginDttm"]),
                            ReqstEndDttm = Convert.ToString(list["reqstEndDttm"]),
                            CanclUseAt = Convert.ToString(list["canclUseAt"]),
                            LctreChargeAmount = Convert.ToString(list["lctreChargeAmount"]),
                            LctrePosblDfk = Convert.ToString(list["lctrePosblDfk"]),
                            LctreBeginDttm = Convert.ToString(list["lctreBeginDttm"]),
                            LctreEndTime = Convert.ToString(list["lctreEndTime"]),
                            LctreRefrnc = Convert.ToString(list["lctreRefrnc"]),
                            FileOrginlNm = Convert.ToString(list["fileOrginlNm"]),
                            Gubun = Convert.ToString(list["gubun"]),
                            ResveGroupNm = Convert.ToString(list["resveGroupNm"]),
                            LctreResveMth = Convert.ToString(list["lctreResveMth"]),
                            LctrePsncpa = Convert.ToString(list["lctrePsncpa"]),
                            ApplyCnt = Convert.ToString(list["applyCnt"]),
                            AdresLo = Convert.ToString(list["adresLo"]),
                            AdresLa = Convert.ToString(list["adresLa"]),
                            Adres = Convert.ToString(list["adres"]),
                            ResidualCNT = Convert.ToString(list["residualCNT"])
                        });
                    }
                    this.DataContext = classList;
                    StsResult.Content = $"OpenAPI {classList.Count}건 조회완료";
                }
            }
            else
            {
                await Commons.ShowMessageAsync("오류", "조건을 선택주세요.");
            }
        }

        private void Chkfree_Checked(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT Id,
                                        LctreNm,
                                        ProgrsSttusNm,
                                        LctreBeginTime,
                                        LctreEndDttm,
                                        ReqstBeginDttm,
                                        ReqstEndDttm,
                                        CanclUseAt,
                                        LctreChargeAmount,
                                        LctrePosblDfk,
                                        LctreBeginDttm,
                                        LctreEndTime,
                                        LctreRefrnc,
                                        FileOrginlNm,
                                        Gubun,
                                        ResveGroupNm,
                                        LctreResveMth,
                                        LctrePsncpa,
                                        ApplyCnt,
                                        AdresLo,
                                        AdresLa,
                                        Adres,
                                        ResidualCNT
                                    FROM classlist
                                    WHERE LctreChargeAmount Like '0' ;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "classlist");
                List<ClassList> classList = new List<ClassList>();
                foreach (DataRow list in ds.Tables["classList"].Rows)
                {
                    classList.Add(new ClassList
                    {
                        Id = 0,
                        LctreNm = Convert.ToString(list["lctreNm"]),
                        ProgrsSttusNm = Convert.ToString(list["progrsSttusNm"]),
                        LctreBeginTime = Convert.ToString(list["lctreBeginTime"]),
                        LctreEndDttm = Convert.ToString(list["lctreEndDttm"]),
                        ReqstBeginDttm = Convert.ToString(list["reqstBeginDttm"]),
                        ReqstEndDttm = Convert.ToString(list["reqstEndDttm"]),
                        CanclUseAt = Convert.ToString(list["canclUseAt"]),
                        LctreChargeAmount = Convert.ToString(list["lctreChargeAmount"]),
                        LctrePosblDfk = Convert.ToString(list["lctrePosblDfk"]),
                        LctreBeginDttm = Convert.ToString(list["lctreBeginDttm"]),
                        LctreEndTime = Convert.ToString(list["lctreEndTime"]),
                        LctreRefrnc = Convert.ToString(list["lctreRefrnc"]),
                        FileOrginlNm = Convert.ToString(list["fileOrginlNm"]),
                        Gubun = Convert.ToString(list["gubun"]),
                        ResveGroupNm = Convert.ToString(list["resveGroupNm"]),
                        LctreResveMth = Convert.ToString(list["lctreResveMth"]),
                        LctrePsncpa = Convert.ToString(list["lctrePsncpa"]),
                        ApplyCnt = Convert.ToString(list["applyCnt"]),
                        AdresLo = Convert.ToString(list["adresLo"]),
                        AdresLa = Convert.ToString(list["adresLa"]),
                        Adres = Convert.ToString(list["adres"]),
                        ResidualCNT = Convert.ToString(list["residualCNT"])
                    });
                }
                this.DataContext = classList;
                StsResult.Content = $"OpenAPI {classList.Count}건 조회완료";
                CboAddress.Text = null;
                CboState.Text = null;
            }
        }


        private void ChkNight_Checked(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT Id,
	                            LctreNm,
	                            ProgrsSttusNm,
	                            LctreBeginTime,
	                            LctreEndDttm,
	                            ReqstBeginDttm,
	                            ReqstEndDttm,
	                            CanclUseAt,
	                            LctreChargeAmount,
	                            LctrePosblDfk,
	                            LctreBeginDttm,
	                            LctreEndTime,
	                            LctreRefrnc,
	                            FileOrginlNm,
	                            Gubun,
	                            ResveGroupNm,
	                            LctreResveMth,
	                            LctrePsncpa,
	                            ApplyCnt,
	                            AdresLo,
	                            AdresLa,
	                            Adres,
	                            ResidualCNT
                            FROM classlist
                            WHERE LctreNm Like '%야간%';";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "classlist");
                List<ClassList> classList = new List<ClassList>();
                foreach (DataRow list in ds.Tables["classList"].Rows)
                {
                    classList.Add(new ClassList
                    {
                        Id = 0,
                        LctreNm = Convert.ToString(list["lctreNm"]),
                        ProgrsSttusNm = Convert.ToString(list["progrsSttusNm"]),
                        LctreBeginTime = Convert.ToString(list["lctreBeginTime"]),
                        LctreEndDttm = Convert.ToString(list["lctreEndDttm"]),
                        ReqstBeginDttm = Convert.ToString(list["reqstBeginDttm"]),
                        ReqstEndDttm = Convert.ToString(list["reqstEndDttm"]),
                        CanclUseAt = Convert.ToString(list["canclUseAt"]),
                        LctreChargeAmount = Convert.ToString(list["lctreChargeAmount"]),
                        LctrePosblDfk = Convert.ToString(list["lctrePosblDfk"]),
                        LctreBeginDttm = Convert.ToString(list["lctreBeginDttm"]),
                        LctreEndTime = Convert.ToString(list["lctreEndTime"]),
                        LctreRefrnc = Convert.ToString(list["lctreRefrnc"]),
                        FileOrginlNm = Convert.ToString(list["fileOrginlNm"]),
                        Gubun = Convert.ToString(list["gubun"]),
                        ResveGroupNm = Convert.ToString(list["resveGroupNm"]),
                        LctreResveMth = Convert.ToString(list["lctreResveMth"]),
                        LctrePsncpa = Convert.ToString(list["lctrePsncpa"]),
                        ApplyCnt = Convert.ToString(list["applyCnt"]),
                        AdresLo = Convert.ToString(list["adresLo"]),
                        AdresLa = Convert.ToString(list["adresLa"]),
                        Adres = Convert.ToString(list["adres"]),
                        ResidualCNT = Convert.ToString(list["residualCNT"])
                    });
                }
                this.DataContext = classList;
                StsResult.Content = $"OpenAPI {classList.Count}건 조회완료";
                CboAddress.Text = null;
                CboState.Text = null;
            }
        }

        private void CboAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chkfree.IsChecked = false;
            ChkNight.IsChecked = false;
        }

        private void CboState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chkfree.IsChecked = false;
            ChkNight.IsChecked = false;
        }

        private void Chkfree_Click(object sender, RoutedEventArgs e)
        {
            Chkfree_Checked(sender, e);
            Chkfree.IsChecked = true;
        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selItem = GrdResult.SelectedItem as ClassList;

            var mapWindow = new MapWindow(Convert.ToDouble(selItem.AdresLo), Convert.ToDouble(selItem.AdresLa));  // 부모창 위치값을 자식창으로 전달
            mapWindow.Owner = this;     // MainWindow 부모
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;    // 부모창 중간에 출력
            mapWindow.ShowDialog();

        }
    }
}

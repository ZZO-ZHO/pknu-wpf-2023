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
                            LctrePsncpa = Convert.ToInt32(list["lctrePsncpa"]),
                            ApplyCnt = Convert.ToInt32(list["applyCnt"]),
                            AdresLo = Convert.ToDouble(list["adresLo"]),
                            AdresLa = Convert.ToDouble(list["adresLa"]),
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
                var query = @"SELECT DATE_FORMAT(Timestamp, '%Y-%m-%d') AS Save_Date
                                  FROM dustsensor
                                 GROUP BY 1
                                 ORDER BY 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Save_Date"]));
                }
                CboAddress.ItemsSource = saveDateList;
            }

        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Chkfree_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkNight_Checked(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            #region < DB 저장 >
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = @"INSERT INTO classlist
                                                (LctreNm,
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
                                                Id)
                                                VALUES
                                                (@LctreNm,
                                                @ProgrsSttusNm,
                                                @LctreBeginTime,
                                                @LctreEndDttm,
                                                @ReqstBeginDttm,
                                                @ReqstEndDttm,
                                                @CanclUseAt,
                                                @LctreChargeAmount,
                                                @LctrePosblDfk,
                                                @LctreBeginDttm,
                                                @LctreEndTime,
                                                @LctreRefrnc,
                                                @FileOrginlNm,
                                                @Gubun,
                                                @ResveGroupNm,
                                                @LctreResveMth,
                                                @Id);";
                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is ClassList)
                        {
                            var item = temp as ClassList;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@LctreNm", item.LctreNm);
                            cmd.Parameters.AddWithValue("@ProgrsSttusNm", item.ProgrsSttusNm);
                            cmd.Parameters.AddWithValue("@LctreBeginTime", item.LctreBeginTime);
                            cmd.Parameters.AddWithValue("@LctreEndDttm", item.LctreEndDttm);
                            cmd.Parameters.AddWithValue("@ReqstBeginDttm", item.ReqstBeginDttm);
                            cmd.Parameters.AddWithValue("@ReqstEndDttm", item.ReqstEndDttm);
                            cmd.Parameters.AddWithValue("@CanclUseAt", item.CanclUseAt);
                            cmd.Parameters.AddWithValue("@LctreChargeAmount", item.LctreChargeAmount);
                            cmd.Parameters.AddWithValue("@LctrePosblDfk", item.LctrePosblDfk);
                            cmd.Parameters.AddWithValue("@LctreBeginDttm", item.LctreBeginDttm);
                            cmd.Parameters.AddWithValue("@LctreEndTime", item.LctreEndTime);
                            cmd.Parameters.AddWithValue("@LctreRefrnc", item.LctreRefrnc);
                            cmd.Parameters.AddWithValue("@FileOrginlNm", item.FileOrginlNm);
                            cmd.Parameters.AddWithValue("@Gubun", item.Gubun);
                            cmd.Parameters.AddWithValue("@ResveGroupNm", item.ResveGroupNm);
                            cmd.Parameters.AddWithValue("@LctreResveMth", item.LctreResveMth);
                            cmd.Parameters.AddWithValue("@Id", item.Id);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await Commons.ShowMessageAsync("저장", $"DB 저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류 {ex.Message}");
            }
            #endregion
        }
    }
}

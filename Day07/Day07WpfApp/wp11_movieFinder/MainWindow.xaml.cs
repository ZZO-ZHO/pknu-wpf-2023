using CefSharp.DevTools.Page;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wp11_movieFinder.Models;
using wp11_MovieFinder.Logics;

namespace wp11_movieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false; // false -> API로 건색해온 결과   true -> 줄겨찾기 보기

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e)
        {
            await Commons.ShowMessageAsync("네이버영화", "네이버영화 사이트로 이동합니다.");
        }

        private async void BtnSearchMovie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                await Commons.ShowMessageAsync("검색", "영화를 검색합니다.");
                return;
            }

            if(TxtMovieName.Text.Length <= 2)
            {
                await Commons.ShowMessageAsync("검색", "검색어를 2자 이상 입력하세요.");
                return;
            }

            try
            {
                SearchMovie(TxtMovieName.Text);
            }
            catch(Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                BtnSearchMovie_Click(sender, e);
            }
        }
        
        // 실제 검색 메서드
        private async void SearchMovie(string movieName)
        {
            string tmdb_apiKey = "91bc89b27eb4908686609d2c8dacb584";
            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUrl = $@"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apiKey}" +
                                $@"&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";
            string result = string.Empty;

            // api 실행 할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // Naver API 요청
            try
            {
                req = WebRequest.Create(openApiUrl);    // URL을 넣어서 객체를 생성 
                res = await req.GetResponseAsync();        // 요청한 결과를 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            //result를 json으로 변경
            var jsonResult = JObject.Parse(result);

            var total = Convert.ToInt32(jsonResult["total_results"]);   // 전체 검색결과수
            // await Commons.ShowMessageAsync("검색결과", total.ToString());
            var items = jsonResult["results"];
            // items를 데이터 그리드에 표시
            var json_array = items as JArray;

            var movieItems = new List<MovieItem>();
            foreach (var val in json_array)
            {
                var MovieItem = new MovieItem()
                {
                    Adult = Convert.ToBoolean(val["adult"]),
                    Id = Convert.ToInt32(val["id"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Overview = Convert.ToString(val["overview"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Poster_Path = Convert.ToString(val["poster_path"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Title = Convert.ToString(val["title"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"])

                };
                movieItems.Add(MovieItem);
            }

            this.DataContext = movieItems;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();
        }

        // 그리드에서 셀선택하면 
        private void GrbResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            { 
                var movie = GrbResult.SelectedItem as MovieItem;
                Debug.WriteLine(movie.Poster_Path);
                if(string.IsNullOrEmpty(movie.Poster_Path))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{movie.Poster_Path}", UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {

            }
        }
        // 영화예고편 유튜브에서 보기
        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrbResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 선택하세요");
                return;
            }

            if (GrbResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 하나만 선택하세요");
                return;
            }

            string movieName = string.Empty;
            var movie = GrbResult.SelectedItem as MovieItem;

            movieName = movie.Title;
            //await Commons.ShowMessageAsync("유튜브", $"예고편 볼영화 {movieName}");
            var trailerWindow = new TrailerWindow(movie);
            trailerWindow.Owner = this;
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //trailerWindow.Show();
            trailerWindow.ShowDialog();
        }

        // 검색결과중에서 좋아하는 영화 저장
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrbResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요(복수선택 가능)");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 영화입니다.");
                return;
            }
            
            List<FavoriteMovieitem> list = new List<FavoriteMovieitem>();
            foreach (MovieItem item in GrbResult.SelectedItems)
            {
                var favoriteMovie = new FavoriteMovieitem()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Original_Title = item.Original_Title,
                    Original_Language = item.Original_Language,
                    Adult = item.Adult,
                    Overview = item.Overview,
                    Release_Date = item.Release_Date,
                    Vote_Average = item.Vote_Average,
                    Popularity = item.Popularity,
                    Poster_Path = item.Poster_Path,
                    Reg_Date = DateTime.Now

                };
                list.Add(favoriteMovie);
            }
            

            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"INSERT INTO  [dbo].[FavoriteMovieitem]
                                             ( [Id]
                                             , [Title]
                                             , [Original_Title]
                                             , [Release_Date]
                                             , [Original_Language]
                                             , [Adult]
                                             , [Popularity]
                                             , [Vote_Average]
                                             , [Poster_Path]
                                             , [Overview]
                                             , [Reg_Date] )
                                       VALUES
                                             ( @Id
                                             , @Title
                                             , @Original_Title
                                             , @Release_Date
                                             , @Original_Language
                                             , @Adult
                                             , @Popularity
                                             , @Vote_Average
                                             , @Poster_Path
                                             , @Overview
                                             , @Reg_Date )";

                    var insRes = 0;
                    foreach (FavoriteMovieitem item in list)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);
                        cmd.Parameters.AddWithValue("@Reg_Date", item.Reg_Date);

                        insRes += cmd.ExecuteNonQuery();
                    }
                    if(list.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공");
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요");
                    }
                    
                }

            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            }
            // DB연동
        }
    }
}

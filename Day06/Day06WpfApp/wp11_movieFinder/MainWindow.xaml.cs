﻿using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private void SearchMovie(string movieName)
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
                res = req.GetResponse();        // 요청한 결과를 응답에 할당
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
                    Id = Convert.ToInt32(val["id"]),
                    Title = Convert.ToString(val["title"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"])
                };
                movieItems.Add(MovieItem);
            }

            this.DataContext = movieItems;
        }
    }
}

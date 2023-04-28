using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTest.Models
{
    public class ClassList
    {

        /*
        "lctreNm":"2023년 상반기 피란학교 천막교실<나도 피란화가!> (5월 6일)",      // 강좌명
        "progrsSttusNm":"대기중",                                                   // 접수상태
        "lctreBeginDttm":"2023-05-06",                                              // 운영기간(시작)
        "lctreEndDttm":"2023-05-06",                                                // 운영기간(종료)
        "reqstBeginDttm":"2023-05-01 09:00:00",                                     // 신청기간(시작)
        "reqstEndDttm":"2023-05-03 18:00:00",                                       // 신청기간(종료)
        "canclUseAt":"Y",                                                           // 취소여부
        "lctreChargeAmount":0,                                                      // 수강료
        "lctrePosblDfk":"6",                                                        // 요일
        "lctreBeginTime":"14:00",                                                   // 시간(시작)
        "lctreEndTime":"16:00",                                                     // 시간(종료)
        "lctreRefrnc":"051-231-6341",                                               // 문의전화
        "fileOrginlNm":"",                                                          // 첨부파일
        "gubun":"1",                                                                // 신청방법
        "resveGroupNm":"임시수도기념관 교육프로그램",                               // 운영기관
        "lctreResveMth":"1"                                                         // 구분
        "lctrePsncpa": 8,                                                           // 정원
        "applyCnt": 0,                                                              // 접수
        "adresLo": "35.103748759",                                                  // 위도
        "adresLa": "129.0175954268",                                                // 경도
        "adres": "부산 서구 임시수도기념로 45 (부민동3가, 임시수도기념관)",         // 주소
        "residualCNT": 8                                                            // 잔여
        */

        public int Id { get; set; }
        public string LctreNm { get; set; }
        public string ProgrsSttusNm { get; set; }
        public string LctreBeginTime { get; set; }
        public string LctreEndDttm { get; set; }
        public string ReqstBeginDttm { get; set; }
        public string ReqstEndDttm { get; set; }
        public string CanclUseAt { get; set; }
        public string LctreChargeAmount { get; set; }
        public string LctrePosblDfk { get; set; }
        public string LctreBeginDttm { get; set; }
        public string LctreEndTime { get; set; }
        public string LctreRefrnc { get; set; }
        public string FileOrginlNm { get; set; }
        public string Gubun { get; set; }
        public string ResveGroupNm { get; set; }
        public string LctreResveMth { get; set; }

        public int LctrePsncpa { get; set; }
        public int ApplyCnt { get; set; }
        public double AdresLo { get; set; }
        public double AdresLa { get; set; }
        public string Adres { get ; set; }
        public string ResidualCNT { get; set;}

    }
}

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wp05_bikeshop.Logics
{
    internal class MyConverter : IValueConverter
    {
        // 대상에다가 표현할때 값을 변환, 표현(Oneway)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() + " km/h";
        }
        
        // 대상값이 바뀌어서 원본의 값을 변환, 표현(Twoway)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) * 3;
        }
    }
}

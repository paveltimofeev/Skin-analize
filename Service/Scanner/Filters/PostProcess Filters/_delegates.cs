﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Service.Scanner.Filters.RegionFilters
{
    public delegate bool IsValidHandler(Point p);
    public delegate void PointActionHandler(int x, int y);
}

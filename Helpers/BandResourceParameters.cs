﻿namespace BandAPI.Helpers
{
    public class BandResourceParameters
    {
        public string MainGenre { get; set; }
        public string SearchQuery { get; set; }

        const int maxPageSize = 3;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 3;
        public int PageSize 
        { 
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value; 
        }
        public string OrderBy { get; set; } = "Name";
        public string Fields { get; set; }
    }
}

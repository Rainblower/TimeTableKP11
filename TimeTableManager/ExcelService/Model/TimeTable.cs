using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTableManager.ExcelService.Model
{
    struct TimeTable
    {
        public string Date;
        public string WeekDay;
        public List<Group> Groups;
    }

    struct Group
    {
        public string GroupName;
        public List<Par> Pars;
    }

    struct Par
    {
        public string ParName;
        public string Cabin;
        public string ParQuantity;
    }
}

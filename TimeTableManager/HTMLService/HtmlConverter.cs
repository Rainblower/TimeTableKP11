using System;
using System.Collections.Generic;
using System.Text;
using TimeTableManager.ExcelService.Model;
using TimeTableManager.MailService;

namespace TimeTableManager.HTMLService
{
    static class HtmlConverter
    {
        public static string _table = StorageManager.ReadFilePath(@"Templates/Table.html");
        public static string _cell = StorageManager.ReadFilePath(@"Templates/Cell.html");
        public static string _doubleCell = StorageManager.ReadFilePath(@"Templates/DoubleCell.html");
        public static string _emptyCell = StorageManager.ReadFilePath(@"Templates/EmptyCell.html");

        private static bool _isOneSkip = false;

        private static List<string> _tables;

        public static List<string> CreateTable(TimeTable timeTable)
        {
            _tables = new List<string>();

            foreach (var group in timeTable.Groups)
            {
                var table = _table;
                var count = 1;
                _isOneSkip = false;

                table = table.Replace("$Date", timeTable.Date);
                table = table.Replace("$Day", timeTable.WeekDay);
                table = table.Replace("$Group", group.GroupName);
                table = table.Replace("$Title", group.GroupName);

                for (int i = 0; i < group.Pars.Count; i += 2)
                {
                    if (_isOneSkip)
                    {
                        _isOneSkip = false;
                        continue;
                    }

                    if (group.Pars.Count % 2 != 0)
                        group.Pars.Remove(group.Pars[group.Pars.Count - 1]);

                    if (group.Pars[i].ParName != group.Pars[i+1].ParName)
                    {
                        var cell = CreateDoubleCell(count, group.Pars[i].ParName, group.Pars[i].Cabin, group.Pars[i+1].ParName, group.Pars[i+1].Cabin);
                        table = table.Replace("$cell" + count, cell);
                        _isOneSkip = true;
                    }
                    else if (group.Pars[i].ParName == null && group.Pars[i+1].ParName == null)
                    {
                        var cell = CreateEmptyCell(count);
                        table = table.Replace("$cell" + count, cell);
                    }
                    else
                    {
                        var cell = CreateCell(count, group.Pars[i].ParName, group.Pars[i].Cabin);
                        table = table.Replace("$cell" + count, cell);
                    }
                    count++;
                }

                table = table.Replace("\r\n", "");
                table = table.Replace("\"", "");

                _tables.Add(table);
            }


            Console.WriteLine("Html converted\n");
            return _tables;
        }

        private static string CreateCell(int count, string parName, string cabin)
        {
            var cell = _cell;

            cell = cell.Replace("$parName", parName);
            cell = cell.Replace("$par", count + " пара");
            cell = cell.Replace("$cabin", cabin);

            return cell;
        }

        private static string CreateDoubleCell(int count, string parName1, string cabin1, string parName2, string cabin2)
        {
            var cell = _doubleCell;

            cell = cell.Replace("$parName1", parName1);
            cell = cell.Replace("$parName2", parName2);
            cell = cell.Replace("$par", count + " пара");
            cell = cell.Replace("$cabin1", cabin1);
            cell = cell.Replace("$cabin2", cabin2);

            return cell;
        }

        private static string CreateEmptyCell(int count)
        {
            var cell = _emptyCell;

            cell = cell.Replace("$par", count + " пара");

            return cell;
        }
    }
}

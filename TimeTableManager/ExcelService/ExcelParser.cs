using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TimeTableManager.ExcelService.Model;

namespace TimeTableManager.ExcelService
{
    class ExcelParser
    {
        private const int SheetIndex = 0;
        private readonly int[] SkipRowArray = new[] { 2, 4, 5, 7 };

        private int _rowCount = 0;
        private int _columnCount = 0;
        private int _blankCount = 0;
        private bool _isSkip = false;

        private List<Group> _groups = new List<Group>();
        private List<Par> _pars;


        public TimeTable Parse(string filePath)
        {
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheetAt(SheetIndex);

            var timeTable = new TimeTable();

            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                _isSkip = false;
                _rowCount++;

                foreach (var skipRow in SkipRowArray)
                {
                    if (_rowCount == skipRow)
                    {
                        _isSkip = true;
                        break;
                    }
                }

                if (_isSkip)
                    continue;

                foreach (var cell in sheet.GetRow(row).Cells)
                {
                    if (cell.ToString().Contains("РАСПИСАНИЕ"))
                    {
                        var values = cell.ToString().Split(" ");
                        var date = values[values.Length - 1];
                        var cultureInfo = new CultureInfo("ru-RU");
                        var weekDay = cultureInfo.DateTimeFormat.GetDayName(DateTime.Parse(date).DayOfWeek);
                        var textInfo = cultureInfo.TextInfo;

                        timeTable.Date = date;
                        timeTable.WeekDay = textInfo.ToTitleCase(weekDay);
                    }

                    if (cell.CellType == CellType.String)
                    {
                        if (_rowCount == 6 && !cell.StringCellValue.Contains("группа"))
                        {
                            var count = 0;
                            _pars = new List<Par>();
                            _blankCount = 0;
                            int _subRow = 7;

                            if (sheet.GetRow(7).Cells[0].StringCellValue != null)
                                _subRow = 7;
                            else
                                _subRow = 15;

                            for (int subRow = _subRow; subRow <= sheet.LastRowNum; subRow++)
                            {
                                foreach (var colCell in sheet.GetRow(subRow).Cells)
                                {
                                    if (colCell.ColumnIndex == cell.ColumnIndex)
                                    {
                                        if (colCell.CellType == CellType.String)
                                        {
                                            count++;
                                            var cellIndex = sheet.GetRow(subRow).Cells.IndexOf(colCell);
                                            var cabinCell = sheet.GetRow(subRow).Cells[cellIndex + 1];
                                            _pars.Add(new Par()
                                            {
                                                ParName = colCell.StringCellValue,
                                                Cabin = cabinCell.ToString(),
                                                ParQuantity = count.ToString()
                                            });
                                            _blankCount = 0;
                                        }
                                        else if (colCell.CellType == CellType.Blank)
                                        {
                                            _blankCount++;
                                            if (_blankCount > 3)
                                            {
                                                count++;
                                                _blankCount = 0;
                                                _pars.Add(new Par()
                                                {
                                                    ParQuantity = count.ToString()
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            _groups.Add(new Group() { GroupName = cell.ToString(), Pars = _pars });
                        }
                    }
                    if (_rowCount > 6)
                    {
                        break;
                    }
                }

                if (_rowCount > 6)
                {
                    break;
                }

            }

            _rowCount = 0;

            timeTable.Groups = _groups;

            Console.WriteLine("Excel parsed\n");
            return timeTable;
        }
    }
}

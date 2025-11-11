using Lab4.DataAccess;

namespace Lab4.DataAccess
{
    public partial class Student
    {
        public double? AvgGrad
        {
            get
            {
                double? avgGrade = null;
                double sum = 0.0;
                foreach (AcademicRecord ac in AcademicRecords)
                {
                    if (ac.Grade.HasValue) sum += ac.Grade.Value;
                }
                if (NumberOfCourses > 0) avgGrade = sum / NumberOfCourses;
                return avgGrade;
            }
        }
        public int NumberOfCourses
        {
            get
            {
                int num = 0;
                foreach (AcademicRecord ac in AcademicRecords)
                {
                    if (ac.Grade.HasValue) num++;
                }
                return num;
            }
        }
        public string DisplayText { get { return Id + " - " + Name; } }
    
    }
}

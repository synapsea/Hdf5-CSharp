using System;

namespace HDF5CSharp.Example.DataTypes
{
    // <summary>
    /// Patient Information class 
    /// </summary>
    public class PatientInfo
    {
        public string PatientFirstName { get; set; }

        public string PatientFamilyName { get; set; }


    }
    /// <summary>
    /// Exam information
    /// </summary>
    public class ProcedureInfo
    {
        /// <summary>
        /// Patient Info
        /// </summary>
        public PatientInfo Patient { get; set; }


        /// <summary>
        /// Date of the Exam
        /// </summary>
        public DateTime ExamDate { get; set; }

        /// <summary>
        /// Unique ID of the current procedure
        /// </summary>
        public string ProcedureID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Procedure { get; set; }

    }
}

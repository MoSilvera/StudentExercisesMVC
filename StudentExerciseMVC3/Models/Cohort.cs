using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public string Designation { get; set; }

        public List<Student> StudentsInCohort { get; set; }
        public List<Instructor> InstructorsInCohort { get; set; }
    }
}

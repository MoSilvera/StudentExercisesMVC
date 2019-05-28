using Microsoft.AspNetCore.Mvc.Rendering;
using StudentExerciseMVC3.Repositories;
using StudentExercisesAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC3.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public Student Student { get; set; } = new Student();

        public List<SelectListItem> Cohorts { get; set; } = new List<SelectListItem>();

        public StudentEditViewModel() { }

        public StudentEditViewModel(int id)
        {
            Student = StudentRepository.GetStudent(id);
            GetAllCohorts(id);
        }

        public void GetAllCohorts(int id)
        {
                    Cohorts = CohortRepository.GetCohorts().Select(li => new SelectListItem
                        {
                            Text = li.Designation,
                            Value = li.Id.ToString()

                        }).ToList();
                    
                    Cohorts.Insert(0, new SelectListItem
                    {
                        Text = "Choose cohort ...",
                        Value = "0"
                    });

                    StudentRepository.GetStudent(id);
               
            
        }
    }
}

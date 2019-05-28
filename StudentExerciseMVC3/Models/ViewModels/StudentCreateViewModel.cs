using Microsoft.AspNetCore.Mvc.Rendering;
using StudentExercisesAPI.Models;
using StudentExerciseMVC3.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC3.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        public Student Student { get; set; } = new Student();

        public List<SelectListItem> Cohorts { get; set; } = new List<SelectListItem>();

        public SqlConnection Connection;

       
        public StudentCreateViewModel()
        {
           
            GetAllCohorts();
        }

        public void GetAllCohorts()
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
                    
                
            
        }
    }
}

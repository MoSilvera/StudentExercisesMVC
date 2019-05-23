using Microsoft.AspNetCore.Mvc.Rendering;
using StudentExercisesAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC3.Models.ViewModels
{
    public class InstructorCreateViewModel
    {
        public Instructor Instructor { get; set; } = new Instructor();

        public List<SelectListItem> Cohorts { get; set; } = new List<SelectListItem>();

        public SqlConnection Connection;

        public InstructorCreateViewModel()
        {

        }
        public InstructorCreateViewModel(SqlConnection connection)
        {
            Connection = connection;
            GetAllCohorts();
        }

        public void GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select c.Id, c.Designation from Cohort c;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Designation = reader.GetString(reader.GetOrdinal("Designation"))
                        };

                        cohorts.Add(cohort);
                    }

                    Cohorts = cohorts.Select(li => new SelectListItem
                        {
                            Text = li.Designation,
                            Value = li.Id.ToString()

                        }).ToList();
                    
                    Cohorts.Insert(0, new SelectListItem
                    {
                        Text = "Choose cohort ...",
                        Value = "0"
                    });
                    reader.Close();
                }
            }
        }
    }
}

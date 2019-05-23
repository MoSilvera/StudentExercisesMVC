using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC3.Models.ViewModels;
using StudentExercisesAPI.Models;

namespace StudentExerciseMVC3.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Students
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT i.Id, 
                        i.FirstName, 
                        i.LastName, 
                        i.SlackHandle, 
                        i.CohortId,
                        i.Specialty,
                        c.Designation 
                        FROM Instructor i Join Cohort c 
                        ON i.CohortId = c.Id;";
                        
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();
                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            Cohort = new Cohort
                            {
                                Designation = reader.GetString(reader.GetOrdinal("Designation")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            }
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return View(instructors);
                }
            }
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT i.Id, 
                        i.FirstName, 
                        i.LastName, 
                        i.SlackHandle, 
                        i.CohortId,
                        i.Specialty,
                        c.Designation 
                        FROM Instructor i Join Cohort c 
                        ON i.CohortId = c.Id
                        Where i.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            Cohort = new Cohort
                            {
                                Designation = reader.GetString(reader.GetOrdinal("Designation")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            }


                        };

                    }
                    reader.Close();
                    return View(instructor);

                }
            }

        }

        // GET: Students/Create
        public ActionResult Create()
        {
            InstructorCreateViewModel model = new InstructorCreateViewModel(Connection);
            return View(model);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] InstructorCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId, Specialty)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstName, @lastName, @slackHandle, @cohortId, @specialty)";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", model.Instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", model.Instructor.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@specialty", model.Instructor.Specialty));



                        int newId = (int)cmd.ExecuteScalar();
                        model.Instructor.Id = newId;
                        
                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            InstructorEditViewModel model = new InstructorEditViewModel(Connection, id);
            return View(model);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, [FromForm] InstructorEditViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Instructor
                                                SET FirstName = @firstName,
                                                    LastName = @lastName,
                                                    SlackHandle = @slackHandle,
                                                    CohortId = @cohortId,
                                                    Specialty = @specialty
                                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstName", Convert.ToString(collection["instructor.FirstName"])));
                        cmd.Parameters.Add(new SqlParameter("@lastName", Convert.ToString(collection["instructor.LastName"])));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", Convert.ToString(collection["instructor.SlackHandle"])));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", Convert.ToInt32(collection["instructor.CohortId"])));
                        cmd.Parameters.Add(new SqlParameter("@specialty", Convert.ToString(collection["instructor.Specialty"])));

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));

                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.CohortId, i.Specialty, c.Designation 
                        FROM Instructor i Join Cohort c 
                        ON i.CohortId = c.Id
                        Where i.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Designation = reader.GetString(reader.GetOrdinal("Designation")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            }


                        };

                    }
                    reader.Close();
                    return View(instructor);

                }
            }
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {   

                    conn.Open();
                   
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Instructor WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));

                    }
                }

                
            }
            catch
            {
                return View();
            }
        }

        private bool StudentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT Id, FirstName, LastName, SlackHandle, CohortId
                            FROM Student
                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
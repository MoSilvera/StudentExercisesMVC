using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC3.Models.ViewModels;
using StudentExerciseMVC3.Repositories;
using StudentExercisesAPI.Models;

namespace StudentExerciseMVC3.Controllers
{
    public class StudentsController : Controller
    {
       

        // GET: Students
        public ActionResult Index()
        {

            var students = StudentRepository.GetStudents();

            return View(students);
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {

            var student = StudentRepository.GetStudent(id);
            return View(student);

        }

        // GET: Students/Create
        public ActionResult Create()
        {
            StudentCreateViewModel model = new StudentCreateViewModel();
            return View(model);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] StudentCreateViewModel model)
        {
            try
            {
                var student = StudentRepository.CreateStudent(model.Student);
                return RedirectToAction(nameof(Index));


            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            StudentEditViewModel model = new StudentEditViewModel(id);
            return View(model);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, [FromForm] StudentEditViewModel model)
        {
            try
            {
                model.Student.Id = id; 
                StudentRepository.UpdateStudent(id, collection);
                return RedirectToAction(nameof(Index));
                
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
            var student = StudentRepository.GetStudent(id);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                if (StudentRepository.DeleteStudent(id))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(Details), new { id = id });
                }


            }
            catch
            {
                return View();
            }
        }

    }
}
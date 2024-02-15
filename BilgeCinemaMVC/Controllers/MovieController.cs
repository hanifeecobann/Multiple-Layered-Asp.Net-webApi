using BilgeCinemaMVC.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;

namespace BilgeCinemaMVC.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _client;
        public MovieController(AppSettings appSettings, HttpClient client)
        {
            _appSettings = appSettings;
            _client = client;
        }
        public async Task<IActionResult> Index()
        {
            var getAllUrl = $"{_appSettings.ApiBaseUrl}/movies";
            var response = await _client.GetFromJsonAsync<List<MovieViewModel>>(getAllUrl);

            return View(response);
        }
        public IActionResult New()
        { 
            return View("Form");
        }
        public async Task<IActionResult> Update(int id)
        {
            var getUrl=$"{_appSettings.ApiBaseUrl}/movies/{id}";
            var viewModel=await _client.GetFromJsonAsync<MovieFormViewModel>(getUrl);
            return View("Form", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(MovieFormViewModel formData)
        {
            if (formData.Id == 0)
            {
                var insertUrl = $"{_appSettings.ApiBaseUrl}/movies";
                
                var response = await _client.PostAsJsonAsync(insertUrl, formData);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Film Kayıt Edilirken Bir Hata Oluştu";
                    return View("form",formData);
                }

            }
            else
            {
                var updateUrl=$"{_appSettings.ApiBaseUrl}/movies/{formData.Id}";
                var response=await _client.PutAsJsonAsync(updateUrl, formData);
                if(response.IsSuccessStatusCode) 
                {
                    return RedirectToAction("Index");
                        
                }
                else
                {
                    ViewBag.ErrorMessage = "Film Yüklenirken bir hata oluştu";
                    return View("Form", formData);
                }
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ChangeDiscount(int id)
        {
            var patchUrl = $"{_appSettings.ApiBaseUrl}/movies/{id}";

            await _client.PatchAsync(patchUrl, null);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int id)
        {
            var deleteUrl = $"{_appSettings.ApiBaseUrl}/movies/{id}";

            await _client.DeleteAsync(deleteUrl);
            return RedirectToAction("Index");

        }

    }
}

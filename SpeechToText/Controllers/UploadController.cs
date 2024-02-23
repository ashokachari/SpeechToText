using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpeechToText.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:5000/upload"; // Replace with your API endpoint

                var completeFilePath="D:\\"+file.FileName;

                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                using (var stream = file.OpenReadStream())
                {
                    content.Add(new StreamContent(stream), "mp3", file.FileName);
                    content.Add(new StringContent(completeFilePath), "file_path"); // Add file path as a parameter

                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully uploaded
                        var data = await response.Content.ReadAsStringAsync();

                        JObject jsonObject = JObject.Parse(data);

                        ViewBag.Message = jsonObject["transcription"];
                    }
                    else
                    {
                        // Handle error
                        ViewBag.Message = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
            }

            return View("Index");
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToText.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string transcriptionMode)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:5000/upload"; // Replace with your API endpoint
                var completeFilePath = "D:\\" + file.FileName;

                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                using (var stream = file.OpenReadStream())
                {
                    content.Add(new StreamContent(stream), "mp3", file.FileName);
                    content.Add(new StringContent(completeFilePath), "file_path"); // Add file path as a parameter

                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(data);

                        var textContent = jsonObject["transcription"];

                        if (transcriptionMode == "file")
                        {
                            // If transcriptionMode is "file", download the file
                            var textFileName = "transcription.txt";
                            var textBytes = Encoding.UTF8.GetBytes(textContent.ToString());
                            var textStream = new MemoryStream(textBytes);

                            // Create a text file response
                            var fileResult = File(textStream, "text/plain", textFileName);

                            // Assign transcription data to ViewBag
                            ViewBag.Message = jsonObject["transcription"];

                            return fileResult;
                        }

                        // If transcriptionMode is "inline", assign transcription data to ViewBag
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

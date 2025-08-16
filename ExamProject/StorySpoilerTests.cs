using ExamProject.Models;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;


namespace Story
{
    [TestFixture]
    [NonParallelizable]

    public class StorySpoilerTests
    {
        private RestClient client;
        private static string? createdStoryId;
        private const string baseUrl = "https://d3s5nxhwblsjbi.cloudfront.net/api";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("ninastory", "ninastory23");

            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/User/Authentication", Method.Post);

            request.AddJsonBody(new { username, password });

            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            return json.GetProperty("accessToken").GetString() ?? string.Empty;

        }


        [Order(1)]
        [Test]
        public void CreatedStory_ShouldReturnCreated()
        {
            var story = new StoryDTO
            {
                Title = "My First Story",
                Description = "This is my new story created.",
                Url = ""
            };

            var request = new RestRequest("/Story/Create", Method.Post);
            request.AddJsonBody(story);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            createdStoryId = json.GetProperty("storyId").GetString() ?? string.Empty;

            Assert.That(createdStoryId, Is.Not.Null.And.Not.Empty, "Food ID should not be null or empty");
            Assert.That(response.Content, Does.Contain("Successfully created!"));
        }

        [Order(2)]
        [Test]
        public void EditCreatedStory_ShouldReturnOK()
        {
            var updatedStory = new StoryDTO
            {
                Title = "My Updated Story",
                Description = "This is my updated story description.",
                Url = ""
            };

            var request = new RestRequest($"/Story/Edit/{createdStoryId}", Method.Put);
            request.AddJsonBody(updatedStory);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Successfully edited"));
        }

        [Order(3)]
        [Test]
        public void GetAllStories_ShouldReturnOK()
        {
            var request = new RestRequest("/Story/All", Method.Get);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var story = JsonSerializer.Deserialize<List<object>>(response.Content);
            Assert.That(story, Is.Not.Empty);
        }

        [Order(4)]
        [Test]
        public void DeleteStory_ShouldReturnOK()
        {
            var request = new RestRequest($"/Story/Delete/{createdStoryId}", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Deleted successfully!"));
        }

        [Order(5)]
        [Test]
        public void CreateStoryWithRequeredFields_BadRequest()
        {
            var invalidStory = new StoryDTO
            {
                Title = "",       
                Description = "", 
                Url = ""          
            };

            var request = new RestRequest("/Story/Create", Method.Post);
            request.AddJsonBody(invalidStory);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Order(6)]
        [Test]
        public void EditNonExistingStory_NotFound()
        {
            var fakeId = "non_existing_story_123";

            var updatedStory = new StoryDTO
            {
                Title = "Fake Title",
                Description = "This story does not exist",
                Url = ""
            };

            var request = new RestRequest($"/Story/Edit/{fakeId}", Method.Put);
            request.AddJsonBody(updatedStory);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content, Does.Contain("No spoilers..."));
        }

        [Order(7)]
        [Test]
        public void DeleteNonExistingStory_BadRequest()
        {
            var fakeId = "non_existing_story_123";

            var request = new RestRequest($"/Story/Delete/{fakeId}", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Does.Contain("Unable to delete this story spoiler!"));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }
    }
}
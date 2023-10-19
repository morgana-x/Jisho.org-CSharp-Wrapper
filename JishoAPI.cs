using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JishoAPI
{
    public class Link
    {
        public string text { get; set; }
        public string url { get; set; }

    }
    public class JapaneseWord
    {
        public string word { get; set; }
        public string reading { get; set; }
    }
    public class EnglishWord
    {
        public List<string> english_definitions { get; set; }
        public List<string> parts_of_speech { get; set; }
        public List<Link> links { get; set; }
        public List<string> tags { get; set; }
        public List<string> restrictions { get; set; }
        public List<string> see_also { get; set; }
        public List<string> antonyms { get; set; }
        public List<string> source { get; set; }
        public List<string> info { get; set; }
    }

    public class Attribution
    {
        public dynamic jmdict { get; set; }
        public dynamic jmnedict { get; set; }
        public dynamic dbpedia { get; set; }


    }
    public class JishoEntry
    {
        public string slug { get; set; }
        public bool is_common { get; set; }
        public List<string> tags { get; set; }
        public List<string> jlpt { get; set; }
        public List<JapaneseWord> japanese { get; set; }
        public List<EnglishWord> senses { get; set; }
        public Attribution attribution { get; set; }
    }

    public class Metadata
    {
        public int status { get; set; }
    }
    public class JishoResult
    {
        public Metadata meta { get; set; }
        public List<JishoEntry> data { get; set; }
    }
    public class JishoAPI
    {
        public string endPoint = "https://jisho.org/api/v1/search/words?keyword=";
        public HttpClient httpClient = new HttpClient();
        public async void Search(string keyword, Action<JishoResult> callback = null)
        {
            HttpResponseMessage response = await httpClient.GetAsync(endPoint + keyword);

            string jsonString = await response.Content.ReadAsStringAsync();

            if (callback == null)
                return;

            JishoResult result = JsonSerializer.Deserialize<JishoResult>(jsonString);


            callback(result);
        }
        public async Task<JishoResult> Search(string keyword)
        {
            return await Search(keyword);
        }
        public void printResults(JishoResult result)
        {
            if (result.meta != null)
                Console.WriteLine("Status: " + result.meta.status.ToString());
            else
                Console.WriteLine("Metadata was null");
            if (result.data == null)
            {
                Console.WriteLine("data was null");
                return;
            }
            foreach (JishoEntry ent in result.data)
            {
                Console.WriteLine(ent.slug);
                if (ent.is_common)
                    Console.WriteLine("- Common Word");
                foreach (string e in ent.jlpt)
                    Console.WriteLine("- " + e);
                foreach (string e in ent.tags)
                    Console.WriteLine("- " + e);
                Console.WriteLine("- Japanese");
                int b = 0;
                foreach (JapaneseWord j in ent.japanese)
                {
                    b++;
                    Console.WriteLine("  - Entry " + b.ToString());
                    Console.WriteLine("   - Kanji: " + j.word);
                    Console.WriteLine("   - Furigana: " + j.reading);
                }
                Console.WriteLine("- English");
                int a = 0;
                foreach (EnglishWord def in ent.senses)
                {
                    a++;
                    Console.WriteLine("  - Entry " + a.ToString());
                    Console.WriteLine("   - Defitions:");
                    foreach (string garbage in def.english_definitions)
                    {
                        Console.WriteLine("    - " + garbage);
                    }
                    Console.WriteLine("   - Forms:");
                    foreach (string garbage in def.parts_of_speech)
                    {
                        Console.WriteLine("    - " + garbage);
                    }
                }
                Console.WriteLine("\n\n\n");
            }
        }
    }
}

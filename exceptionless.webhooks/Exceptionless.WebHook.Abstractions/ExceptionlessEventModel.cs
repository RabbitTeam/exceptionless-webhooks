using Newtonsoft.Json;
using System;

namespace Exceptionless.WebHook.Abstractions
{
    public class ExceptionlessEventModel
    {
        public string Id { get; set; }
        public string Url { get; set; }

        [JsonProperty("occurrence_date")]
        public DateTime OccurrenceDate { get; set; }

        public string Type { get; set; }
        public string Message { get; set; }

        [JsonProperty("project_id")]
        public string ProjectId { get; set; }

        [JsonProperty("project_Name")]
        public string ProjectName { get; set; }

        [JsonProperty("organization_id")]
        public string OrganizationId { get; set; }

        [JsonProperty("organization_name")]
        public string OrganizationName { get; set; }

        [JsonProperty("stack_id")]
        public string StackId { get; set; }

        [JsonProperty("stack_url")]
        public string StackUrl { get; set; }

        [JsonProperty("stack_title")]
        public string StackTitle { get; set; }

        [JsonProperty("stack_tags")]
        public string[] StackTags { get; set; }

        [JsonProperty("total_occurrences")]
        public int TotalOccurrences { get; set; }

        [JsonProperty("first_occurrence")]
        public DateTime FirstOccurrence { get; set; }

        [JsonProperty("last_occurrence")]
        public DateTime LastOccurrence { get; set; }

        [JsonProperty("is_new")]
        public bool IsNew { get; set; }

        [JsonProperty("is_regression")]
        public bool IsRegression { get; set; }

        [JsonProperty("is_critical")]
        public bool IsCritical { get; set; }
    }
}
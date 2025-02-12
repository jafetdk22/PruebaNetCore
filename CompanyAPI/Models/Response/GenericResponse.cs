namespace CompanyAPI.Models.Response
{

    public class GenericResponse
    {
        public string Message { get; set; } = string.Empty;
        public string[] Errors { get; set; } = new string[0];
        public bool Success { get; set; } = true;
        public int SuccessCount { get; set; } = 0; 
    }
}

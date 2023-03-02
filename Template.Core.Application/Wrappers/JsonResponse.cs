namespace Template.Core.Application.Wrappers
{
    public class Response<T>
    {
        public Response(T data, string message = "Successed Request")
        {
            Data = data;
            Message = message;
            Successed = true;
        }

        public Response(string message)
        {
            Message = message;
            Successed = false;
        }

        public T Data { get; set; }
        public bool Successed { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
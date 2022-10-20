namespace MVC_01.Models
{
    public class Summernote
    {
        public Summernote(string iDEditor, bool loadLibrary = true)
        {
            IDEditor = iDEditor;
            LoadLibrary = loadLibrary;
        }

        public string IDEditor { get; set; }
        public bool LoadLibrary { get; set; }
        public int height { set; get; } = 120;
        public string toolbar { get; set; } = @"[
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['height', ['height']],
                ['insert', ['link', 'picture', 'video','elfinder']],
                ['view', ['fullscreen', 'codeview', 'help']]
            ]";

    }
}
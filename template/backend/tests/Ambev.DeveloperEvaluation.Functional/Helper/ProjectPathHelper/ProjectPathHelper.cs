namespace Ambev.DeveloperEvaluation.Functional.Helper.ProjectPathHelper
{
    public static class ProjectPathHelper
    {
        public static string? FindProjectPath(string startDirectory, string projectFileName)
        {
            Console.WriteLine($"Procurando {projectFileName} a partir de: {startDirectory}");
            var dir = new DirectoryInfo(startDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, projectFileName);
                Console.WriteLine($"=> Verificando: {candidate}");
                if (File.Exists(candidate))
                    return dir.FullName;
                dir = dir.Parent;
            }
            return null;
        }
    }
}

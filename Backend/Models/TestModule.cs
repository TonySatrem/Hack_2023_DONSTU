namespace Backend.Models
{
    public class TestModule
    {
        public int Id { get; set; }
        public string? categoryTest { get; set; }
        public string? levelTest { get; set; }

        public void Update(TestModule module)
        {
            if (module.categoryTest != null)
                categoryTest = module.categoryTest;
            if (module.levelTest != null)
                levelTest = module.levelTest;
        }
    }
}

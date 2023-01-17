namespace EmmasEngines.Data
{
    public class EmmasEnginesInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            EmmasEnginesContext context = applicationBuilder.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<EmmasEnginesContext>();

            //Initilize Data
        }
    }
}

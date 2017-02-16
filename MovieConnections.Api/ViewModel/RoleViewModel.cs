namespace MovieConnections.Api.ViewModel {
    public class RoleViewModel
    {
        public RoleViewModel()
        { }
        public RoleViewModel(dynamic model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
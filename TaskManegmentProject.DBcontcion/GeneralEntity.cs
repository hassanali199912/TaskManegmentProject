﻿namespace TaskManegmentProject.DBcontcion
{
    public class GeneralEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Tournament.Core.ViewModels.Tournament
{
    public class NewParticipant
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

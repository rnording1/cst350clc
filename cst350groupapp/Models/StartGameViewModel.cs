using System.ComponentModel.DataAnnotations;

namespace cst350groupapp.Models
{
    public class StartGameViewModel
    {
        //must be greater than 0
        [Range(1, int.MaxValue)]
        [Required]
        public int Rows { get; set; }

        //must be greater than 0
        [Range(1, int.MaxValue)]
        [Required]
        public int Columns { get; set; }

        //Difficulty is a percentage of cells that are live 0-100
        [Range(0, 100)]
        [Required]
        public double Difficulty { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;


namespace Core.DTO;

public class RegisterDTO
{
    [Required(ErrorMessage = "The 'Email' Can't Be Blank!!!")]
    [StringLength(60, ErrorMessage = "The 'Email' Can't Be More Than 60 Characters")]
    [EmailAddress(ErrorMessage = "The 'Email' is not in a Correct Format")]
    public string Email { get; set; }


    [StringLength(maximumLength:11, MinimumLength = 11, ErrorMessage = "The 'Phone Number' must be 11 Characters")]
    [RegularExpression("^0[0-9]{10}$", ErrorMessage = "The Number Should be '0XXXXXXXXXX'")] // exp: 09001234567
    [DataType(DataType.PhoneNumber)]
    public string Phone { get; set; }


    [Required(ErrorMessage = "Password can't be blank")]
    [DataType(DataType.Password)]
    public string Password { get; set; }


    [Required(ErrorMessage = "Confirm Password can't be blank")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
    public string ConfirmPassword { get; set; }
}
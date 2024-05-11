using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountsController(UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager,
			ITokenService tokenService,
			IMapper mapper)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_mapper = mapper;
		}

		//Register
		[HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			if (CheckEmailExists(model.Email).Result.Value)
				return BadRequest(new ApiResponse(400, "Email Is Already In Use"));

			var appUser = new AppUser()
			{
				Email = model.Email,
				DisplayName = model.DisplayName,
				PhoneNumber = model.PhoneNumber,
				UserName = model.Email.Split('@')[0]
			};

			var result = await _userManager.CreateAsync(appUser, model.Password);

			if (!result.Succeeded) return BadRequest(new ApiResponse(400));

			var returnedUser = new UserDto()
			{
				DisplayName = appUser.DisplayName,
				Email = appUser.Email,
				Token = await _tokenService.CreateTokenAsync(appUser, _userManager)
			};

			return Ok(returnedUser);
		}

		//Login
		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var appUser = await _userManager.FindByEmailAsync(model.Email);

			//Email is not registered before
			if (appUser is null) return Unauthorized(new ApiResponse(401));

			var result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false);

			//Wrong password
			if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

			return Ok(new UserDto()
			{
				DisplayName = appUser.DisplayName,
				Email = appUser.Email,
				Token = await _tokenService.CreateTokenAsync(appUser, _userManager)
			}); 
		}

		//Get Current User
		[Authorize]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(email);

			var returnedUser = new UserDto()
			{
				Email = user.Email,
				DisplayName = user.DisplayName,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			};

			return Ok(returnedUser);
		}

		//Get Current User Address
		[Authorize]
		[HttpGet("Address")]
		public async Task<ActionResult<AddressDto>> GetCurrentAddress()
		{
			var user = await _userManager.FindUserWithAddressAsync(User);

			var mappedAddress = _mapper.Map<AddressDto>(user.Address);

			return Ok(mappedAddress);
		}

		//Update Address
		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto updatedAddress)
		{
			var user = await _userManager.FindUserWithAddressAsync(User);
			var mappedAddress = _mapper.Map<Address>(updatedAddress);
			mappedAddress.Id = user.Address.Id;
			user.Address = mappedAddress;
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded) return BadRequest(new ApiResponse(400));
			return Ok(updatedAddress);
		}

		[HttpGet("emailExists")]
		public async Task<ActionResult<bool>> CheckEmailExists(string email)
			=> await _userManager.FindByEmailAsync(email) is not null;
	}
}

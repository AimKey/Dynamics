using Dynamics.DataAccess.Repository;
using Dynamics.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using Dynamics.Models.Models;
using Dynamics.Models.Models.Dto;
using Dynamics.Models.Models.ViewModel;
using Dynamics.Utility.Mapper;

namespace Dynamics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepo;
        private readonly IRequestRepository _requestRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IProjectResourceRepository _projectResourceRepo;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepo, IRequestRepository requestRepo,
            IProjectRepository projectRepo, IOrganizationRepository organizationRepo,
            IProjectResourceRepository projectResourceRepo, IMapper mapper)
        {
            _logger = logger;
            _userRepo = userRepo;
            _requestRepo = requestRepo;
            _projectRepo = projectRepo;
            _organizationRepo = organizationRepo;
            _projectResourceRepo = projectResourceRepo;
            _mapper = mapper;
        }

        // Landing page
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Homepage()
        {
            List<Request> requests = await _requestRepo.GetAllRequestWithUsersAsync();
            List<Project> projects = await _projectRepo.GetAllAsync();
            // Map to view model for display
            List<ProjectOverviewDto> projectOverviewDtos = new List<ProjectOverviewDto>();
            foreach (var p in projects)
            {
                ProjectOverviewDto dto = await MapToProjectOverviewDto(p);
                projectOverviewDtos.Add(dto);
            }

            // TODO: For organization, wait for Tuan
            var result = new HomepageViewModel
            {
                Requests = requests,
                Projects = projectOverviewDtos
            };
            
            return View(result);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<ProjectOverviewDto> MapToProjectOverviewDto(Project p)
        {
            // TODO: Use mapper instead
            var testMappedObject = _mapper.Map<ProjectOverviewDto>(p);
            var tempProjectOverviewDto = new ProjectOverviewDto();
            tempProjectOverviewDto.ProjectName = p.ProjectName;
            tempProjectOverviewDto.ProjectUser = (await _userRepo.GetAsync(u => u.UserID == p.LeaderID)).UserFullName;
            tempProjectOverviewDto.ProjectId = p.ProjectID;
            tempProjectOverviewDto.ProjectLocation = "Not implemented";
            tempProjectOverviewDto.ProjectMembers = _projectRepo.CountMemberOfProjectById(p.ProjectID);
            tempProjectOverviewDto.ProjetDescription = p.ProjectDescription;
            tempProjectOverviewDto.ProjectProgress = _projectRepo.GetProjectProgressById(p.ProjectID);
            var moneyRaised = await _projectResourceRepo.GetAsync(pr => pr.ResourceName == "Money" && pr.ProjectID == p.ProjectID);
            if (moneyRaised != null)
            {
                tempProjectOverviewDto.ProjectRaisedMoney = moneyRaised.Quantity ?? 0;
            }
            tempProjectOverviewDto.ProjectAttachment = p.Attachment;
            tempProjectOverviewDto.ProjectStatus = p.ProjectStatus;

            return tempProjectOverviewDto;
        }
    }
}
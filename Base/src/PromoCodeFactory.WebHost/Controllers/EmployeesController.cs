using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetAllAsync(cancellationToken);

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeDto()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return Ok(employeeModel);
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveEmployeeByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (employee != null)
            {
                await _employeeRepository.RemoveByIdAsync(employee.Id, cancellationToken);
                return NoContent();
            }
            return NotFound();

        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto employeeDto, CancellationToken cancellationToken)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Email = employeeDto.Email,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName
            };

            var newEmployee = await _employeeRepository.AddAsync(employee, cancellationToken);

            var dto = new EmployeeDto
            {
                FullName = newEmployee.FullName,
                Email = newEmployee.Email,
                Id = newEmployee.Id
            };

            return Created($"{Request.GetDisplayUrl()}/{dto.Id}", dto);
        }

        /// <summary>
        /// Обновить сотрудника
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployeeAsync(Guid id, CreateEmployeeDto employeeDto, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (employee != null)
            {
                employee.FirstName = employeeDto.FirstName;
                employee.Email = employeeDto.Email;
                employee.LastName = employeeDto.LastName;

                var updatedEmployee = await _employeeRepository.UpdateByIdAsync(id, employee, cancellationToken);

                var dto = new EmployeeDto
                {
                    FullName = updatedEmployee.FullName,
                    Email = updatedEmployee.Email,
                    Id = updatedEmployee.Id
                };

                return Ok(dto);
            }

            return NotFound();
        }
    }
}
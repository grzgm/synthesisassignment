using LogicLayer.InterfacesManagers;
using LogicLayer.InterfacesRepository;
using LogicLayer.Models;

namespace LogicLayer.Managers
{
	public class EmployeeManager : IEmployeeManager
	{
		private readonly IEmployeeRepository employeeRepository;

		public EmployeeManager(IEmployeeRepository employeeRepository)
		{
			this.employeeRepository = employeeRepository
				?? throw new ArgumentNullException(nameof(employeeRepository));
		}

		Employee IEmployeeManager.ReadEmployeeByEmailPassword(string email, string password)
		{
			try
			{
				return new Employee(employeeRepository.ReadEmployeeByEmailPassword(email, password));
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}

using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface IAnimalService
{


	Task<AnimalGroup> AddAnimalGroupAsync(
		long userId,
		string type,
		string name,
		int quantity,
		AnimalSex animalSex);


	Task<AnimalProduct> AddAnimalProductAsync(
		long userId,
		long animalGroupId,
		string name,
		double quantity,
		AnimalProductUnit unit,
		TimeSpan periodTaken
		);

	Task<AnimalProduct> GetAnimalProductAsync(long farmerId, long animalProductId);

	Task<IQueryBuilder<AnimalGroup>> GetAnimalGroupAsync(long AccountId, long animalGroupId);

	Task<IQueryBuilder<AnimalGroup>> GetAllAnimalGroupAsync(long AccountId, long userId);



	Task<AnimalProduct> UpdateAnimalProductAsync(long userId, AnimalProduct animalProduct);

	Task DeleteAnimalProductAsync(long userId, long animalProductId);

	

	Task<AnimalGroup> UpdateAnimalGroupAsync(long userId, AnimalGroup animalGroup);
	 
	

	Task DeleteAnimalGroupAsync(long userId, long animalGroupId);





}
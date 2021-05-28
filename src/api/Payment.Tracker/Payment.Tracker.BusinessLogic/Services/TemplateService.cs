using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Template;
using Payment.Tracker.BusinessLogic.Mappers;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly IGenericRepository<PaymentPositionTemplate> _positionTemplateRepo;

        public TemplateService(IGenericRepository<PaymentPositionTemplate> positionTemplateRepo)
        {
            _positionTemplateRepo = positionTemplateRepo;
        }

        public async Task<PaymentSetTemplateDto> GetTemplateAsync()
        {
            List<PaymentPositionTemplateDto> templatePositions = await _positionTemplateRepo.GetAsAsync(
                template => PaymentPositionTemplateMapper.ToDto(template));

            return new PaymentSetTemplateDto
            {
                Positions = templatePositions
            };
        }

        public async Task<IServiceActionResult<PaymentSetTemplateDto>> UpsertTemplateAsync(PaymentSetTemplateDto dto)
        {
            List<PaymentPositionTemplate> existingPositions = await _positionTemplateRepo.GetAllAsync();
            if (existingPositions.Count > 0)
            {
                existingPositions.ForEach(x => _positionTemplateRepo.Delete(x));
            }

            var newPositionsMapped = dto
                .Positions
                .Select(PaymentPositionTemplateMapper.ToModel)
                .ToList();

            await _positionTemplateRepo.InsertManyAsync(newPositionsMapped);
            await _positionTemplateRepo.SaveChangesAsync();

            return ServiceActionResult<PaymentSetTemplateDto>.GetCreated(
                new PaymentSetTemplateDto
                {
                    Positions = newPositionsMapped.Select(PaymentPositionTemplateMapper.ToDto).ToList()
                }
            );
        }
    }
}
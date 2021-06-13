using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Template;
using Payment.Tracker.BusinessLogic.Mappers;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

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
            List<PaymentPositionTemplateDto> templatePositions = await _positionTemplateRepo.GetAllAsAsync(
                template => PaymentPositionTemplateMapper.ToDto(template),
                new Filter<PaymentPositionTemplate>());

            return new PaymentSetTemplateDto
            {
                Positions = templatePositions
            };
        }

        public async Task<IServiceActionResult<PaymentSetTemplateDto>> UpsertTemplateAsync(PaymentSetTemplateDto dto)
        {
            List<string> existingIds = await _positionTemplateRepo.GetAllAsAsync(
                    t => t.Id, new Filter<PaymentPositionTemplate>());
            if (existingIds.Count > 0)
            {
                await _positionTemplateRepo.DeleteAsync(existingIds); }

            var newPositionsMapped = dto
                .Positions
                .Select(PaymentPositionTemplateMapper.ToModel)
                .ToList();

            await _positionTemplateRepo.InsertManyAsync(newPositionsMapped);
            
            return ServiceActionResult<PaymentSetTemplateDto>.GetCreated(
                new PaymentSetTemplateDto
                {
                    Positions = newPositionsMapped.Select(PaymentPositionTemplateMapper.ToDto).ToList()
                }
            );
        }
    }
}
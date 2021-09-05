using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Common.Responses;
using Watch.Functions.Entities;

namespace Watch.Functions.Functions
{
    class ConsolidatedApi
    {

        [FunctionName(nameof(Consolidated))]
        public static async Task<IActionResult> Consolidated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            log.LogInformation("Get all consolidated received.");

            string Filter = TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.Equal, false);
            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>().Where(Filter);
            TableQuerySegment<TimeEntity> timesNotConsolidated = await timeTable.ExecuteQuerySegmentedAsync(query, null);
            int add = 0;
            int update = 0;

            List<IGrouping<int, TimeEntity>> timesByEmployee = (from t in timesNotConsolidated group t by t.EmployeeId).ToList();
            foreach (IGrouping<int, TimeEntity> groupTime in timesByEmployee)
            {
                TimeSpan dif;
                double minutes = 0;
                List<TimeEntity> orderedList = groupTime.OrderBy(x => x.Date).ToList();
                int duo = 0;
                if (orderedList.Count % 2 == 0)
                {
                    duo= orderedList.Count;
                }
                else
                {
                    duo = orderedList.Count - 1;
                }
                
                TimeEntity[] vecTimes = orderedList.ToArray();

                if (duo == 2)
                {
                    add += 1;
                    update += 0;
                }
                else if (duo > 2)
                {
                    add += 1;
                    update += (duo - 2)/2;
                }
                
                try
                {
                    for (int i = 0; i < duo; i++)
                    {
                       
                        if (i % 2 != 0 && vecTimes.Length > 1)
                        {
                            minutes = 0;
                            dif = vecTimes[i].Date - vecTimes[i - 1].Date;
                            minutes += dif.TotalMinutes;
                            TableQuery<ConsolidatedEntity> consolidatedQuery = new TableQuery<ConsolidatedEntity>();
                            TableQuerySegment<ConsolidatedEntity> allConsolidated = await consolidatedTable.ExecuteQuerySegmentedAsync(consolidatedQuery, null);
                            IEnumerable<ConsolidatedEntity> employee = allConsolidated.Where(x => x.EmployeeId == vecTimes[i].EmployeeId);
                            if (employee.Count() == 0 )
                            {
                                ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                                {
                                    EmployeeId = vecTimes[i].EmployeeId,
                                    Date = DateTime.Today,
                                    MinutesWork = (int)minutes,
                                    ETag = "*",
                                    PartitionKey = "CONSOLIDATED",
                                    RowKey = vecTimes[i].RowKey
                                };
                                TableOperation addConsolidatedOperation = TableOperation.Insert(consolidatedEntity);
                                await consolidatedTable.ExecuteAsync(addConsolidatedOperation);
                                
                                
                            }
                            else
                            {
                                TableOperation findOp = TableOperation.Retrieve<ConsolidatedEntity>("CONSOLIDATED", employee.First().RowKey);
                                TableResult findRes = await consolidatedTable.ExecuteAsync(findOp);
                                ConsolidatedEntity consolidatedEntity = (ConsolidatedEntity)findRes.Result;
                                consolidatedEntity.MinutesWork += (int)minutes;
                                consolidatedEntity.Date = employee.First().Date;
                                TableOperation addConsolidatedOperation = TableOperation.Replace(consolidatedEntity);
                                await consolidatedTable.ExecuteAsync(addConsolidatedOperation);
                                
                                
                            }
                            TableOperation findOperation = TableOperation.Retrieve<TimeEntity>("TIME", vecTimes[i].RowKey);
                            TableResult findResult = await timeTable.ExecuteAsync(findOperation);
                            TimeEntity timeEntity = (TimeEntity)findResult.Result;
                            timeEntity.IsConsolidated = true;
                            TableOperation addOperation = TableOperation.Replace(timeEntity);
                            await timeTable.ExecuteAsync(addOperation);
                        }
                    }
                }
                catch (Exception e)
                {
                    string error = e.Message;
                    
                }
            }
            string message = $"Consolidation summary. Records added: {add}, records updated: {update}.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = null
            });
        }

       

        
    }
}

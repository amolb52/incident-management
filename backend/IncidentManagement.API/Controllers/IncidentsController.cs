using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using IncidentManagement.API.Data;
using IncidentManagement.API.Models;

namespace IncidentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<IncidentsController> _logger;

    public IncidentsController(ApplicationDbContext db, BlobServiceClient blobServiceClient, ILogger<IncidentsController> logger)
    {
        _db = db;
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.Incidents.Include(i => i.Attachments).AsNoTracking().OrderByDescending(i => i.CreatedAt);
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, items });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var incident = await _db.Incidents.Include(i => i.Attachments).FirstOrDefaultAsync(i => i.Id == id);
        if (incident == null) return NotFound();
        return Ok(incident);
    }

    [HttpPost]
    [RequestSizeLimit(50_000_000)] // allow up to ~50MB for prototype
    public async Task<IActionResult> Create([FromForm] string title, [FromForm] string? description, [FromForm] string severity = "Medium", [FromForm] IFormFileCollection? files = null)
    {
        if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title is required");
        var incident = new Incident { Title = title, Description = description, Severity = Enum.Parse<Severity>(severity), Status = Status.Open };

        if (files != null && files.Count > 0)
        {
            var container = _blobServiceClient.GetBlobContainerClient("attachments");
            await container.CreateIfNotExistsAsync();
            foreach (var file in files)
            {
                var id = Guid.NewGuid();
                var blobName = $"{incident.Id}/{id}_{file.FileName}";
                var blob = container.GetBlobClient(blobName);
                await using var stream = file.OpenReadStream();
                await blob.UploadAsync(stream, overwrite: true);
                var attachment = new Attachment { Id = id, FileName = file.FileName, BlobUrl = blob.Uri.ToString(), ContentType = file.ContentType ?? "application/octet-stream", Size = file.Length };
                incident.Attachments.Add(attachment);
            }
        }

        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync();

        // Trigger notification (placeholder) - for prototyping we just log; in production call an Azure Function or queue
        _logger.LogInformation("New incident created: {Id}", incident.Id);

        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var incident = await _db.Incidents.FirstOrDefaultAsync(i => i.Id == id);
        if (incident == null) return NotFound();
        incident.Status = Enum.Parse<Status>(status);
        incident.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(incident);
    }
}

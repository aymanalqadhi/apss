﻿namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a survey
/// </summary>
public sealed class Survey : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the survey
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the expiration date of the survey
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the survey
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the end month of the survey
    /// </summary>
    public int EndMonth { get; set; }

    /// <summary>
    /// Gets or sets the end day of the survey
    /// </summary>
    public int EndDay { get; set; }

    /// <summary>
    /// Gets or sets the user who created the survey
    /// </summary>
    public User CreatedBy { get; set; } = null!;
}
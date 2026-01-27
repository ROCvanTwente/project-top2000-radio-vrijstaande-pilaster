using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Data;

namespace TemplateJwtProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dalers")]
    public async Task<IActionResult> GrootsteDalers(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_GrootsteDalers",
                reader => new PositieVerschilDto(
                    GetInt(reader, "Position"),
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetInt(reader, "Delta"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")
                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load biggest drops", error = ex.Message });
        }
    }

    [HttpGet("stijgers")]
    public async Task<IActionResult> GrootsteStijgers(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_GrootsteStijgers",
                reader => new PositieVerschilDto(
                    GetInt(reader, "Position"),
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetInt(reader, "Delta"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")

                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load biggest rises", error = ex.Message });
        }
    }

    [HttpGet("alleedities")]
    public async Task<IActionResult> AlleEdities()
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_AlleEdities",
                reader => new AlleEditiesDto(
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")
                )
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load ever-present songs", error = ex.Message });
        }
    }

    [HttpGet("nieuw")]
    public async Task<IActionResult> NieuwBinnen(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_NieuwBinnen",
                reader => new PositieNummerDto(
                    GetInt(reader, "Position"),
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")
                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load new entries", error = ex.Message });
        }
    }

    [HttpGet("verdwenen")]
    public async Task<IActionResult> Verdwenen(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_Verdwenen",
                reader => new VerdwenenDto(
                    GetInt(reader, "PreviousPosition"),
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")
                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load dropouts", error = ex.Message });
        }
    }

    [HttpGet("opnieuwbinnen")]
    public async Task<IActionResult> OpnieuwBinnen(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_OpnieuwBinnen",
                reader => new OpnieuwBinnenDto(
                    GetInt(reader, "Position"),
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId"),
                    GetInt(reader, "LastYearInList"),
                    GetInt(reader, "YearBack"),
                    GetInt(reader, "YearsOut")
                   
                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load re-entries", error = ex.Message });
        }
    }

    [HttpGet("dezelfdeplek")]
    public async Task<IActionResult> DezelfdePlek(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_DezelfdePlek",
                reader => new PositieNummerDto(
                    GetInt(reader, "Position"),
                    GetString(reader, "Title"),
                    GetString(reader, "Artist"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")
                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load unchanged positions", error = ex.Message });
        }
    }

    [HttpGet("achterelkaar")]
    public async Task<IActionResult> AchterElkaar(int year)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_AchterElkaar",
                reader => new AchterElkaarDto(
                    GetString(reader, "Artist"),
                    GetInt(reader, "Position"),
                    GetString(reader, "Title"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetNullableInt(reader, "NextPosition"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")

                ),
                new SqlParameter("@Year", year)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load consecutive artist positions", error = ex.Message });
        }
    }

    [HttpGet("eenkeer")]
    public async Task<IActionResult> EenJaar()
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_EenJaar",
                reader => new EenjaarDto(
                    GetString(reader, "Artist"),
                    GetString(reader, "Title"),
                    GetNullableInt(reader, "ReleaseYear"),
                    GetInt(reader, "Position"),
                    GetNullableInt(reader, "Year"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")
                    

                )
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load one-timers", error = ex.Message });
        }
    }

    [HttpGet("topartiesten")]
    public async Task<IActionResult> TopArtiesten(int year, int take = 3)
    {
        try
        {
            var result = await RunStoredProcAsync(
                "sp_Top2000_TopArtiesten",
                reader => new TopArtiestenDto(
                    GetString(reader, "Artist"),
                    GetInt(reader, "SongCount"),
                    GetDouble(reader, "AveragePosition"),
                    GetInt(reader, "BestPosition"),
                    GetString(reader, "ImgUrl"),
                    GetInt(reader, "SongId")

                ),
                new SqlParameter("@Year", year),
                new SqlParameter("@Take", take)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load top artists", error = ex.Message });
        }
    }

    private async Task<List<T>> RunStoredProcAsync<T>(string procName, Func<DbDataReader, T> map, params SqlParameter[] parameters)
    {
        var results = new List<T>();
        var connection = _context.Database.GetDbConnection();

        try
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command = connection.CreateCommand();
            command.CommandText = procName;
            command.CommandType = CommandType.StoredProcedure;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return results;
    }

    private static int GetInt(DbDataReader reader, string name)
    {
        return reader[name] is DBNull ? 0 : Convert.ToInt32(reader[name]);
    }

    private static int? GetNullableInt(DbDataReader reader, string name)
    {
        return reader[name] is DBNull ? null : Convert.ToInt32(reader[name]);
    }

    private static double GetDouble(DbDataReader reader, string name)
    {
        return reader[name] is DBNull ? 0 : Convert.ToDouble(reader[name]);
    }

    private static string GetString(DbDataReader reader, string name)
    {
        return reader[name] is DBNull ? string.Empty : reader[name].ToString() ?? string.Empty;
    }
}

public record PositieVerschilDto(int Position, string Title, string Artist, int? ReleaseYear, int Delta, string ImgUrl, int SongId);

public record AlleEditiesDto(string Title, string Artist, int? ReleaseYear, string ImgUrl, int SongId);

public record PositieNummerDto(int Position, string Title, string Artist, int? ReleaseYear, string ImgUrl, int SongId);

public record VerdwenenDto(int PreviousPosition, string Title, string Artist, int? ReleaseYear, string ImgUrl, int SongId);

public record AchterElkaarDto(string Artist, int Position, string Title, int? ReleaseYear, int? NextPosition, string ImgUrl, int SongId);

public record EenjaarDto(string Artist, string Title, int? ReleaseYear, int Position, int? Top2000Year, string ImgUrl, int SongId);

public record TopArtiestenDto(string Artist, int SongCount, double AveragePosition, int BestPosition, string ImgUrl, int SongId);

public record OpnieuwBinnenDto(int Position, string Title, string Artist, int? ReleaseYear, string ImgUrl, int SongId, int LastYearInList, int YearBack, int YearsOut);

using Prometheus;

namespace TournamentBracket.Infrastructure.Metrics;

public class MetricsRegistry
{
	public readonly Counter ChangeTablesTypes;

	public MetricsRegistry()
	{
		ChangeTablesTypes = Prometheus.Metrics.CreateCounter("change_tables_types", "Change tables types");
	}
}

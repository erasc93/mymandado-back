namespace api_mandado.models;

public record CartChangeEvent(int CartId, string Action, string ByUser);

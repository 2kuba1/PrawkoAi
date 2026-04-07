namespace Application.Models;

public record GeminiResponse(List<Candidate> Candidates);
    
public record Candidate(Content Content);

public record Content(List<Part> Parts);

public record Part(string Text);

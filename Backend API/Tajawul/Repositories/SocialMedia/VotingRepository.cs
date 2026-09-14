using Neo4j.Driver;
using Tajawul.Models.Enums;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Services;

namespace Tajawul.Repositories.SocialMedia;

public partial class VotingRepository(Neo4jService neo4jService)
{
    private readonly Neo4jService _neo4JService = neo4jService;

    public async Task<VotingResult?> ToggleVoting(VotingEnum nodeType, VotingRelationshipEnum relationshipType, string nodeId, string userId)
    {
        string intendedRelType, oppositeRelType;
        string intendedCounter, oppositeCounter;
        bool isAttemptingUpvote;


        if (relationshipType == VotingRelationshipEnum.UPVOTED)
        {
            intendedRelType = "UPVOTED";
            oppositeRelType = "DOWNVOTED";
            intendedCounter = "upvoteCount";
            oppositeCounter = "downvoteCount";
            isAttemptingUpvote = true;
        }
        else // DOWNVOTED
        {
            intendedRelType = "DOWNVOTED";
            oppositeRelType = "UPVOTED";
            intendedCounter = "downvoteCount";
            oppositeCounter = "upvoteCount";
            isAttemptingUpvote = false;
        }

        // Ensure nodeType is passed as a string for dynamic label usage
        var nodeTypeString = nodeType.ToString();

        return await _neo4JService.ExecuteReadAsync(
            $@"
        MATCH (u:User {{id: $userId}})
        MATCH (n:{nodeTypeString} {{id: $nodeId}}) // Use parameterized nodeTypeString here

        // Check for existing intended relationship (e.g., UPVOTED if current action is upvote)
        OPTIONAL MATCH (u)-[r_intended:{intendedRelType}]->(n)

        // Check for existing opposite relationship (e.g., DOWNVOTED if current action is upvote)
        OPTIONAL MATCH (u)-[r_opposite:{oppositeRelType}]->(n)

        // Pass all necessary dynamic strings and entities to APOC
        WITH u, n, r_intended, r_opposite,
             $intendedRelType AS intendedRelStr,
             $oppositeRelType AS oppositeRelStr,
             $intendedCounter AS intendedCounterStr,
             $oppositeCounter AS oppositeCounterStr

        CALL apoc.do.case([
            // Case 1: Intended relationship exists (e.g., already upvoted, user clicks upvote again -> un-upvote)
            r_intended IS NOT NULL,
            'DETACH DELETE r_intended
             SET n.' + intendedCounterStr + ' = coalesce(n.' + intendedCounterStr + ', 1) - 1 // Decrement intended
             RETURN ""removed_intended"" AS action_taken', // Return a status or dummy value

            // Case 2: Intended relationship does NOT exist, but opposite DOES (e.g., was downvoted, user clicks upvote -> switch vote)
            r_intended IS NULL AND r_opposite IS NOT NULL,
            'DETACH DELETE r_opposite // Remove opposite relationship
             SET n.' + oppositeCounterStr + ' = coalesce(n.' + oppositeCounterStr + ', 1) - 1 // Decrement opposite counter
             WITH u, n, intendedRelStr, intendedCounterStr // Pass u, n, and dynamic strings for creation
             CALL apoc.create.relationship(u, intendedRelStr, {{createdAt: timestamp()}}, n) YIELD rel AS new_intended_rel
             SET n.' + intendedCounterStr + ' = coalesce(n.' + intendedCounterStr + ', 0) + 1 // Increment intended counter
             RETURN ""switched_to_intended"" AS action_taken',

            // Case 3: Neither relationship exists (e.g., no vote, user clicks upvote -> add upvote)
            r_intended IS NULL AND r_opposite IS NULL,
            'CALL apoc.create.relationship(u, intendedRelStr, {{createdAt: timestamp()}}, n) YIELD rel AS new_intended_rel
             SET n.' + intendedCounterStr + ' = coalesce(n.' + intendedCounterStr + ', 0) + 1 // Increment intended counter
             RETURN ""added_intended"" AS action_taken'
            ],
            // Else: Should ideally not be reached if logic is exhaustive. Can be an empty string or error indicator.
            'RETURN ""no_action"" AS action_taken',
            {{
                u: u, n: n, r_intended: r_intended, r_opposite: r_opposite,
                intendedRelStr: intendedRelStr, oppositeRelStr: oppositeRelStr,
                intendedCounterStr: intendedCounterStr, oppositeCounterStr: oppositeCounterStr
            }}
        ) YIELD value 
    
        // After modifications, return the updated counts from the node
        // Initialize counts if they don't exist to prevent null issues later
            SET n.upvoteCount = coalesce(n.upvoteCount, 0)
    
            SET n.downvoteCount = coalesce(n.downvoteCount, 0)
    

            RETURN
    
                n.upvoteCount AS Upvotes,
                n.downvoteCount AS Downvotes,
                value.action_taken AS actionTaken // Pass the action_taken string
            ",

            new
            {
                userId,
                nodeId,
                nodeTypeString, // Pass the string version of nodeType
                intendedRelType, // e.g., "UPVOTED"
                oppositeRelType, // e.g., "DOWNVOTED"
                intendedCounter, // e.g., "upvoteCount"
                oppositeCounter  // e.g., "downvoteCount"
            },
            async result =>
                {
                    var records = await result.ToListAsync();
                    var record = records.FirstOrDefault();
                    if (record == null) return null;

                    string actionTaken = record["actionTaken"].As<string>() ?? "no_action";
                    bool finalIsUpvoted = false;
                    bool finalIsDownvoted = false;

                    if (isAttemptingUpvote)
                    {
                        if (actionTaken == "added_intended" || actionTaken == "switched_to_intended")
                        {
                            finalIsUpvoted = true;
                        }
                    }
                    else
                    {
                        if (actionTaken == "added_intended" || actionTaken == "switched_to_intended")
                        {
                            finalIsDownvoted = true;
                        }
                    }

                    return new VotingResult
                    {
                        Upvotes = record["Upvotes"]?.As<int?>(),
                        Downvotes = record["Downvotes"]?.As<int?>(),
                        IsUpvotedByCurrentUser = finalIsUpvoted,
                        IsDownvotedByCurrentUser = finalIsDownvoted
                    };
                }
            );
    }
}





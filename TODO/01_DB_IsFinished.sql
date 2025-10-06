ALTER TABLE dbo.[Match] ADD IsFinished bit NOT NULL CONSTRAINT DF_Match_IsFinished DEFAULT (0);

ALTER TABLE dbo.[Match]
ADD PairA AS (CASE WHEN Player1Id <= Player2Id THEN Player1Id ELSE Player2Id END) PERSISTED,
    PairB AS (CASE WHEN Player1Id <= Player2Id THEN Player2Id ELSE Player1Id END) PERSISTED;

CREATE UNIQUE INDEX UX_Match_ActiveUniquePairPerTE
ON dbo.[Match](TournamentEventId, PairA, PairB)
WHERE IsFinished = 0;
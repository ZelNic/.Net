Создание таблицы.

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MachinePosition')
BEGIN
    CREATE TABLE MachinePosition (
        Id int IDENTITY(1,1) PRIMARY KEY,
        positionX float,
        positionY float
    )
END

INSERT INTO MachinePosition (positionX, positionY)
VALUES 
    (54.9029846573357, 83.94408051288354),
    (53.9029846573357, 82.94408051288354),
    (55.9029846573357, 84.94408051288354)




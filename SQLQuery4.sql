CREATE TABLE ServiceRequests (
    RequestID INT PRIMARY KEY IDENTITY(1,1), 
    ServiceType VARCHAR(100),                
    FeesPerPageRM DECIMAL(10, 2),            
    Quantity INT,                            
    CustomerID INT,                          
    TotalPriceRM DECIMAL(10, 2),             
    UrgentRequest BIT DEFAULT 0,             
    Status VARCHAR(50),                      
    AssignedWorkerID INT                     
);
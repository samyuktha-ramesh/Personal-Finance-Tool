## Personal-Finance-Tool

### Running the Program
Type `dotnet run` to start the program.

### Commands
Type `[date] [amount] [source]` : to enter a transaction. For example: 50 indicates an income and -50 indicates an expense. Not Entering a year defaults it to the current year.
Type `balance` : to check current balance.
Type `view [month]` : to view all transactions. Passing with no filters will show total balance.
Type `delete [date] [amount] [category]` : to delete transaction.
Type `edit` and follow instructions : to edit a transaction.
Type `end` to close program.

### Example: Adding an Income
`$ 8/7 2000 job`

### Example: Adding an expense
`$ 8/7 -300 restaurant`

### Example: Deleting a transaction
`$delete 8/7 2000 job`


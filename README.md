## Personal-Finance-Tool

### Running the Program
Type `dotnet run` to start the program.

### Commands
`[date] [amount] [source]` : to enter a transaction. For example: 50 indicates an income and -50 indicates an expense. Not Entering a year defaults it to the current year.<br />
`balance` : to check current balance.<br />
`view [month]` : to view all transactions. Passing with no filters will show total balance.<br />
`delete [date] [amount] [category]` : to delete transaction.<br />
`edit` and follow instructions : to edit a transaction.<br />
`end` to close program.

### Example: Adding an Income
`$ 8/7 2000 job`

### Example: Adding an expense
`$ 8/7 -300 restaurant`

### Example: Deleting a transaction
`$ delete 8/7 2000 job`


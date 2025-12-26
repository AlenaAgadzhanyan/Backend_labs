using FluentMigrator;

[Migration(3)]
public class AddStatusToOrderTable : Migration
{
    public override void Up()
    {
        var sql = @"
            alter table orders add column status text not null default 'new';

            drop type if exists v1_order;
            create type v1_order as (
                id bigint,
                customer_id bigint,
                delivery_address text,
                total_price_cents bigint,
                total_price_currency text,
                created_at timestamp with time zone,
                updated_at timestamp with time zone,
                status text
            );
        ";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}

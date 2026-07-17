
class Class {
  public id: string | null;
  public name: string;
  public tuition: number;

  public constructor(id: string,name: string, tuition: number) {
    this.id = id;
    this.name = name;
    this.tuition = tuition;
  }
}

export default Class;